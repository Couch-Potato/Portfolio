// NeuralVisualizer.h : Include file for standard system include files,
// or project specific include files.


#pragma once


#include <iostream>
#include <vector>
#include <fstream>
#include <sstream>
#include <string>
#include <map>
#include "rapidcsv.h"
#include <algorithm>
#include "StripVisualizer.h"

#define USE_PIXEL_STRIPS false

#include "rpi_ws281x/ws2811.h"



#define PLAYTYPE_BEGIN 0x00
#define PLAYTYPE_END 0x01
#define PLAYTYPE_PULSE 0x02

#define NOTE_GAMMA 0x00
#define NOTE_THETA 0x01
#define NOTE_BETA 0x02
#define NOTE_ALPHA 0x03
#define NOTE_EX 0x04


struct WaveData {
    float Delta;
    float Theta;
    float Alpha1;
    float Alpha2;
    float Beta1;
    float Beta2;
    float Gamma1;
    float Gamma2; 
    float Time;
    uint32_t FrameId;
};

class DataSource {
    public:
    virtual void Attach() = 0;
    virtual WaveData Step() {
        return WaveData{};
    }
};

struct Note
{
    uint8_t Type;
    uint16_t NoteId;
    uint8_t Note;
};

uint16_t GetNoteId();

struct ProcessedData {
    std::vector<Note> Notes;
    float THETA;
    float ALPHA;
    float BETA;
    float GAMMA;
};

class Processor {
    public:
    virtual void Attach(){}
    virtual ProcessedData Step(WaveData data) {};
};

class Visualizer{
    public:
        virtual void Attach(){};
        virtual void Step(ProcessedData data){};
};

struct ItemColor
{
    unsigned char R;
    unsigned char G;
    unsigned char B;
};

struct LEDApplicator
{
    unsigned char R;
    unsigned char G;
    unsigned char B;
    float Delay;
    float fallOff;
    std::chrono::time_point<std::chrono::system_clock> start_time;
};

class LED
{
public:
    unsigned char R;
    unsigned char G;
    unsigned char B;
    unsigned char X;
    unsigned char Y;
    unsigned char Z;
    uint32_t ToPackedInt();
    void Set(unsigned char R, unsigned char G, unsigned char B, float fallOff, float delay, char blendMode = 0);
    void Update();
    std::vector<LEDApplicator> applicators;
};

class LEDEngine
{
public:
    std::vector<LED> led;
    void Set(uint32_t ledId, uint32_t value);
    LEDEngine(uint8_t ledCount);
    LEDEngine();
    void Update();
};

class StripVisualizer : public Visualizer
{
    

public:
    LEDEngine engine;
    void Step(ProcessedData);
    void Attach();
    StripVisualizer();
};

class CsvSource : public DataSource {
    public:
        WaveData Step();
        virtual void Attach();
};

class StandardProcessor : public Processor {
    public:
        ProcessedData Step(WaveData data);
};

// class AudioVisualizer : public Visualizer {
//     public:
//         void Step(ProcessedData);
//         void Attach();
// };
class VisHost
{
public:
    void Begin()
    {
        Source.Attach();
        Processor.Attach();
        //vis2.Attach();
        vis3.Attach();
        for (auto x : Visualizers)
            x.Attach();
    }
    void Step()
    {
        LastWaveData = Source.Step();
        LastPData = Processor.Step(LastWaveData);
        //vis2.Step(LastPData);
        vis3.Step(LastPData);
        // for (auto v : Visualizers)
        // {
        //     v.Step(LastPData);
        // }
    }
    std::vector<Visualizer> Visualizers;
    CsvSource Source;
    //AudioVisualizer vis2;
    StripVisualizer vis3;
    StandardProcessor Processor;
    ProcessedData LastPData;
    WaveData LastWaveData;
};

static VisHost *VIS_CREATE(StandardProcessor p, CsvSource d)
{
    auto host = new VisHost();
    host->Source = d;
    host->Processor = p;
    host->Begin();
    return host;
}

static void VIS_DESTROY(VisHost *host)
{
    delete host;
}

static void VIS_STEP(VisHost *host)
{
    host->Step();
}

static ProcessedData VIS_GET_PROCESSED_DATA(VisHost *host)
{
    return host->LastPData;
}

static WaveData VIS_GET_WAVE_DATA(VisHost *host)
{
    return host->LastWaveData;
}

static void VIS_ATTACH_VISUALIZER(VisHost *host, Visualizer v)
{
    host->Visualizers.push_back(v);
    v.Attach();
}

// static void VIS_LOAD_SOUND(sf::Sound *sound, sf::SoundBuffer* buffer, std::string name) {
//     if (!buffer->loadFromFile(name))
//     {
//         std::cout << "Failed to load: " << name;
//     }
//     sound->setBuffer(*buffer);
// }

static int RAND_INT(int max)
{
    return rand() % (max+1);
}

// class AudioLibrary {
//     public:
//         std::vector<sf::Sound> Sounds;
//         std::vector<sf::SoundBuffer> Buffers;
//         void AddSound(std::string name){
//             sf::Sound sound;
//             sf::SoundBuffer buf;
//             Sounds.push_back(sound);
//             Buffers.push_back(buf);

//             VIS_LOAD_SOUND(&Sounds[Sounds.size() - 1], &Buffers[Sounds.size() - 1], "./Note/" + name);
//         }
//         void PlayRandom(){
//             Sounds[RAND_INT(Sounds.size() -1)].play();
//         }
// };