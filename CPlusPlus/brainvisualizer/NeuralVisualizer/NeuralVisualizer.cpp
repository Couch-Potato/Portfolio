// NeuralVisualizer.cpp : Defines the entry point for the application.
//
#define TARGET_FREQ WS2811_TARGET_FREQ
#define GPIO_PIN 10
#define DMA 10
#define STRIP_TYPE WS2811_STRIP_RGB
#define ON_COLOR 0x00ffe1
#define BLOOD_SPEED 60
#include "NeuralVisualizer.h"

// sf::Sound BG_ALPHA;
// sf::SoundBuffer __BUF_ALPHA;
// sf::Sound BG_BETA;
// sf::SoundBuffer __BUF_BETA;
// sf::Sound BG_GAMMA;
// sf::SoundBuffer __BUF_GAMMA;
// sf::Sound BG_THETA;
// sf::SoundBuffer __BUF_THETA;
float lastTime = 0;
float timeToWait = 0;
std::chrono::time_point<std::chrono::system_clock> start;

float getTimeSince(std::chrono::time_point<std::chrono::system_clock> timepoint) {
    std::chrono::duration<float> timeSince = std::chrono::system_clock::now() - timepoint;
    return timeSince.count();
}

ws2811_t ledstring =
    {
        .freq = TARGET_FREQ,
        .dmanum = DMA,
        .channel =
            {
                [0] =
                    {
                        .gpionum = GPIO_PIN,
                        .invert = 0,
                        .count = NUM_LEDS,
                        .strip_type = STRIP_TYPE,
                        .brightness = 255,
                    },
                [1] =
                    {
                        .gpionum = 0,
                        .count = 0,
                        .brightness = 0,
                    },
            },
};

ws2811_led_t *matrix;

int main()
{
    ws2811_return_t ret;
    if ((ret = ws2811_init(&ledstring)) != WS2811_SUCCESS)
    {
        fprintf(stderr, "ws2811_init failed: %s\n", ws2811_get_return_t_str(ret));
        return ret;
    }
    auto host = VIS_CREATE(StandardProcessor(), CsvSource());
    //VIS_ATTACH_VISUALIZER(host, AudioVisualizer());

   // sf::RenderWindow window(sf::VideoMode(800,600), "Visualizer");

    while (true)
    {

        for (int i = 0; i < NUM_LEDS; i++)
        {
            ledstring.channel[0].leds[i] = 0x0;
        }
        // sf::Event event;
        // while (window.pollEvent(event))
        // {
        //     // "close requested" event: we close the window
        //     if (event.type == sf::Event::Closed)
        //         window.close();
        // }

        // window.clear(sf::Color::Black);


        if (timeToWait <= 0) {
            VIS_STEP(host);
            timeToWait = (VIS_GET_WAVE_DATA(host).Time-lastTime);
        }

        timeToWait -= getTimeSince(start);
        start = std::chrono::system_clock::now();
        host->vis3.engine.Update();


        lastTime = VIS_GET_WAVE_DATA(host).Time;

        for (int i = 0; i < host->vis3.engine.led.size(); i++)
        {
            ledstring.channel[0].leds[i] = host->vis3.engine.led[i].ToPackedInt();
        }
        if ((ret = ws2811_render(&ledstring)) != WS2811_SUCCESS)
        {
            fprintf(stderr, "ws2811_render failed: %s\n", ws2811_get_return_t_str(ret));
        }
            // for (int i = 0; i < host->vis3.engine.led.size(); i++) {
            //     sf::RectangleShape rectangle(sf::Vector2f(50.0f, 50.0f));
            //     auto xc = host->vis3.engine.led[i];
            //     rectangle.setFillColor(sf::Color(xc.R, xc.G, xc.B));
            //     auto originY = ((i - (i % 16)) / 16) * 50;
            //     rectangle.move((i%16) * 50, ((i - (i % 16)) / 16) * 50);
            //     window.draw(rectangle);
            // }

            // window.display();
    }

    // // Loop while the sound is playing
    // while (sound.getStatus() == sf::Sound::Playing)
    // {
    //     // Leave some CPU time for other processes
    //     sf::sleep(sf::milliseconds(100));

    //     // Display the playing position
    //     std::cout << "\rPlaying... " << sound.getPlayingOffset().asSeconds() << " sec        ";
    //     std::cout << std::flush;
    // }
}

using std::cerr;
using std::cout;
using std::endl;
using std::ifstream;
using std::istringstream;
using std::ostringstream;
using std::string;

string readFileIntoString(const string &path)
{
    auto ss = ostringstream{};
    ifstream input_file(path);
    if (!input_file.is_open())
    {
        cerr << "Could not open the file - '"
             << path << "'" << endl;
        exit(EXIT_FAILURE);
    }
    ss << input_file.rdbuf();
    return ss.str();
}

bool is_number(string str)
{
    for (int i = 0; i < str.size(); i++)
    {
        if ((int)str[i] < 10)
        {
            return true;
        }
        else
        {
            return true;
        }
    }
}

uint32_t currentStep = 0;
WaveData currentData{};
std::map<int, std::vector<float>> csvData;
rapidcsv::Document doc;

// Please kill me...
static float magic(string in) {
    if (is_number(in)) {
        return std::stof(in);
    }else {
        return 0.0f;
    }
}

WaveData CsvSource::Step(){
    if (doc.GetRowCount() < currentStep + 2) {
        return {};
    }
    currentData.Time = magic(doc.GetRow<string>(currentStep)[1]);
    currentData.FrameId = magic(doc.GetRow<string>(currentStep)[0]);
    currentData.Delta = magic(doc.GetRow<string>(currentStep)[2]);
    currentData.Theta = magic(doc.GetRow<string>(currentStep)[3]);
    currentData.Alpha1 = magic(doc.GetRow<string>(currentStep)[4]);
    currentData.Alpha2 = magic(doc.GetRow<string>(currentStep)[5]);
    currentData.Beta1 = magic(doc.GetRow<string>(currentStep)[6]);
    currentData.Beta2 = magic(doc.GetRow<string>(currentStep)[7]);
    currentData.Gamma1 = magic(doc.GetRow<string>(currentStep)[8]);
    currentData.Gamma2 = magic(doc.GetRow<string>(currentStep)[9]);
    currentStep++;

    return currentData;
}

uint16_t GetNoteId(){
    return rand() % 0xffffffff;
}


void CsvSource::Attach() {
    doc = rapidcsv::Document("data.csv");
}


struct VerboseWaveData {
    bool IsIncreasing;
    bool IsDecreasing;
    bool IsPeak;
    float PeakSize;
    float PeakLength;
    float PeakBegin;
    float IncreaseTime;
    float DecreaseTime;
    float PeakBeginValue;
    float Value;
    float Slope;
    float AROC;
};

WaveData previous{0};

void PrepareVerboseWave(float a, float b, VerboseWaveData* data, float t, VerboseWaveData previous) {
    float slope = (a-b) / t;
    if (slope > 0)
    {
        data->IsIncreasing = true;
        data->IsDecreasing = false;
    }
    else
    {
        data->IsIncreasing = false;
        data->IsDecreasing = true;
    }
    data->Value = a;
    data->Slope = slope;
    
    data->PeakBegin = previous.PeakBegin;
    data->PeakBeginValue = previous.PeakBeginValue;

    if (previous.IsDecreasing && data->IsIncreasing) {
        data->PeakBegin = t;
        data->IncreaseTime = t;
        data->PeakBeginValue = a;
    }
    if (previous.IsIncreasing && data->IsDecreasing) {
        data->PeakLength = t - previous.PeakBegin;
        data->PeakSize = b - previous.PeakBeginValue;
        data->IsPeak = true;
    }

    data->AROC = ((currentStep * (previous.AROC-1)) + a) / currentStep;  
    
}

void PrepareVerboseWave(float a, float b, VerboseWaveData *data, float t)
{
    float slope = (a - b) / t;
    if (slope > 0)
    {
        data->IsIncreasing = true;
        data->IsDecreasing = false;
    }
    else
    {
        data->IsIncreasing = false;
        data->IsDecreasing = true;
    }
    data->Value = a;
    data->Slope = slope;

    data->AROC = (a + b)/2;
}

struct VerboseWaveDataSet {
    VerboseWaveData ALPHA;
    VerboseWaveData BETA;
    VerboseWaveData THETA;
    VerboseWaveData GAMMA;
};

void PrepareVerboseWaveSet(WaveData a, WaveData b, VerboseWaveDataSet *set){
    PrepareVerboseWave(a.Alpha1, b.Alpha1,&set->ALPHA, a.Time - b.Time);
    PrepareVerboseWave(a.Beta1, b.Beta1, &set->BETA, a.Time - b.Time);
    PrepareVerboseWave(a.Gamma1, b.Gamma1, &set->GAMMA, a.Time - b.Time);
    PrepareVerboseWave(a.Theta, b.Theta, &set->THETA, a.Time - b.Time);
}

void PrepareVerboseWaveSet(WaveData a, WaveData b, VerboseWaveDataSet *set, VerboseWaveDataSet old)
{
    PrepareVerboseWave(a.Alpha1, b.Alpha1, &set->ALPHA, a.Time - b.Time, old.ALPHA);
    PrepareVerboseWave(a.Beta1, b.Beta1, &set->BETA, a.Time - b.Time, old.BETA);
    PrepareVerboseWave(a.Gamma1, b.Gamma1, &set->GAMMA, a.Time - b.Time, old.GAMMA);
    PrepareVerboseWave(a.Theta, b.Theta, &set->THETA, a.Time - b.Time, old.THETA);
}

ProcessedData data{};
VerboseWaveDataSet prevSet{};

ProcessedData StandardProcessor::Step(WaveData in) {
    if (previous.FrameId == 0) {
        previous = in;
        return ProcessedData{};
    }

    VerboseWaveDataSet vdata{};

    if (prevSet.ALPHA.AROC == 0) {
        PrepareVerboseWaveSet(in, previous, &vdata);
        prevSet = vdata;
        return ProcessedData{};
    }else {
        PrepareVerboseWaveSet(in, previous, &vdata, prevSet);
        prevSet = vdata;
    }
    
    

    data.ALPHA = vdata.ALPHA.AROC / 25704;
    data.BETA = vdata.BETA.AROC / 25704;
    data.GAMMA = vdata.GAMMA.AROC / 25704;
    data.THETA = vdata.THETA.AROC / 257040;

    if (vdata.ALPHA.IsPeak) {
        Note note{};
        note.Note = 0x00;
        note.NoteId = GetNoteId();
        note.Type = NOTE_ALPHA;
        if (vdata.ALPHA.PeakSize > 3000) {
            data.Notes.push_back(note);
        }
       
    }
    if (vdata.BETA.IsPeak)
    {
        Note note{};
        note.Note = 0x00;
        note.Type = NOTE_BETA;
        note.NoteId = GetNoteId();
        if (vdata.BETA.PeakSize > 3000)
        {
            data.Notes.push_back(note);
        }
    }
    if (vdata.THETA.IsPeak)
    {
        Note note{};
        note.Note = 0x00;
        note.Type = NOTE_THETA;
        note.NoteId = GetNoteId();
        if (vdata.THETA.PeakSize > 3000)
        {
            data.Notes.push_back(note);
        }
    }
    if (vdata.GAMMA.IsPeak)
    {
        Note note{};
        note.Note = 0x00;
        note.Type = NOTE_GAMMA;
        note.NoteId = GetNoteId();
        if (vdata.GAMMA.PeakSize > 3000)
        {
            data.Notes.push_back(note);
        }
    }

    previous = in;

    return data;
}

// AudioLibrary L_ALPHA;
// AudioLibrary L_BETA;
// AudioLibrary L_THETA;
// AudioLibrary L_GAMMA;

// void AudioVisualizer::Step(ProcessedData data){
//     BG_ALPHA.setVolume(data.ALPHA*50);
//     BG_BETA.setVolume(data.BETA*50);
//     BG_THETA.setVolume(data.THETA*50);
//     BG_GAMMA.setVolume(data.GAMMA*50);

//     for (auto n : data.Notes) {
//         if (n.Type == NOTE_ALPHA)
//             L_ALPHA.PlayRandom();
//         if (n.Type == NOTE_BETA)
//             L_BETA.PlayRandom();
//         if (n.Type == NOTE_THETA)
//             L_THETA.PlayRandom();
//         if (n.Type == NOTE_GAMMA)
//             L_GAMMA.PlayRandom();
//     }

// }

// void AudioVisualizer::Attach(){
//     VIS_LOAD_SOUND(&BG_ALPHA, &__BUF_ALPHA, "./Long/ALPHA_1.wav");
//     VIS_LOAD_SOUND(&BG_BETA, &__BUF_BETA, "./Long/BETA_1.wav");
//     VIS_LOAD_SOUND(&BG_THETA, &__BUF_THETA, "./Long/THETA_1.wav");
//     VIS_LOAD_SOUND(&BG_GAMMA, &__BUF_GAMMA, "./Long/GAMMA_1.wav");

//     BG_ALPHA.setLoop(true);
//     BG_BETA.setLoop(true);
//     BG_THETA.setLoop(true);
//     BG_GAMMA.setLoop(true);

//     L_ALPHA.AddSound("ALPHA_1.wav");
//     L_ALPHA.AddSound("ALPHA_2.wav");
//     L_ALPHA.AddSound("ALPHA_3.wav");

//     L_BETA.AddSound("BETA_1.wav");
//   //  L_BETA.AddSound("BETA_2.wav");
//    // L_BETA.AddSound("BETA_3.wav");

//     L_GAMMA.AddSound("GAMMA_1.wav");
//     L_GAMMA.AddSound("GAMMA_2.wav");
//     L_GAMMA.AddSound("GAMMA_3.wav");
//     L_GAMMA.AddSound("GAMMA_4.wav");

//     L_THETA.AddSound("THETA_1.wav");
//     //L_THETA.AddSound("THETA_2.wav");

//     BG_ALPHA.play();
//     BG_BETA.play();
//     BG_THETA.play();
//     BG_GAMMA.play();
// }