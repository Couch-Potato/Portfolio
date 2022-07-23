
#include <algorithm>
#pragma once
#include "NeuralVisualizer.h"

ItemColor SelectedColor{};

void PLAY_NOTE_GAMMA(LEDEngine* engine){
    // All flash white
    for (auto light : engine->led) {
        light.Set(0xff,0xff,0xff, 2.0f, 0.0f);
    }
}

void PLAY_NOTE_THETA(LEDEngine* engine) {
    for (int i = 0; i < NUM_RANDOM; i++) {
        int randomSelected = rand() % (engine->led.size() - 1);
        engine->led[randomSelected].Set(255, 0, 0, 1.0f, i / 4.0f);
    }
}

void PLAY_NOTE_BETA(LEDEngine *engine)
{
    for (int i = 0; i < engine->led.size(); i++) {
        engine->led[i].Set(0, 255, 0, .25f, i/20.0f);
    }
}

void PLAY_NOTE_ALPHA(LEDEngine *engine)
{
    for (int i = engine->led.size() - 1; i >= 0; i--)
    {
        engine->led[i].Set(0, 0, 255, 1.0f, i / 5.0f);
    }
}

int clamp(int v, int min, int max) {
    if (v > max) return max;
    if (v < min) return min;
    return v;
}

StripVisualizer::StripVisualizer(){
    int x = NUM_LEDS;
    engine = LEDEngine(x);
}

void StripVisualizer::Step(ProcessedData data) {
    for (auto const& note : data.Notes) {
        if (note.Type == NOTE_ALPHA) {
            PLAY_NOTE_ALPHA(&engine);
        }
        if (note.Type == NOTE_GAMMA)
        {
            PLAY_NOTE_GAMMA(&engine);
        }
        if (note.Type == NOTE_BETA)
        {
            PLAY_NOTE_BETA(&engine);
        }
        if (note.Type == NOTE_THETA)
        {
            PLAY_NOTE_THETA(&engine);
        }
    }
    //engine.Update();
}

void LED::Set(unsigned char R, unsigned char G, unsigned char B, float fallOff, float delay, char blendMode)
{
    LEDApplicator appl{
        R,
        G,
        B,
        delay,
        fallOff
    };
    appl.start_time = std::chrono::system_clock::now();
    applicators.push_back(appl);
}

void LED::Update() {
    R = 0;
    G = 0;
    B = 0;
    std::vector<uint16_t> flagged;
    int i = 0;
    for (int j = 0; j<applicators.size();j++) {
        if (applicators[i].Delay <= 0 && applicators[i].fallOff > 0)
        {
            // Do falloff
            R = clamp(R + applicators[i].R, 0, 255);
            G = clamp(G + applicators[i].G, 0, 255);
            B = clamp(B + applicators[i].B, 0, 255);

            auto current = std::chrono::system_clock::now();
            std::chrono::duration<float> elapsedTotal = current - applicators[i].start_time;
            applicators[i].fallOff = applicators[i].fallOff - elapsedTotal.count();
            applicators[i].start_time = current;
        }
        if (applicators[i].Delay <= 0 && applicators[i].fallOff <= 0)
        {
            // Flag this item for removal
            flagged.push_back(i);
        }
        if (applicators[i].Delay > 0)
        {
            // We just need to wait for the delay
            auto current = std::chrono::system_clock::now();
            std::chrono::duration<float> elapsedTotal = current - applicators[i].start_time;
            applicators[i].Delay -= elapsedTotal.count();
            applicators[i].start_time = current;
        }
        i++;
    }
    for (auto const item : flagged) {
        applicators.erase(applicators.begin() + item);
        for (int j = 0; j < flagged.size(); j++) {
            flagged[j] -= 1;
        }
    }
}

uint32_t LED::ToPackedInt(){
    return (R << 16 | G << 8 | B);
}

void StripVisualizer::Attach()
{
    SelectedColor.G = 0;
    SelectedColor.B = 254;
    SelectedColor.R = 254;
}

LEDEngine::LEDEngine(){

}

LEDEngine::LEDEngine(uint8_t ledCount){
    led.resize(ledCount);
    int z = 0;
    for (int i = 0; i < led.size(); i++) {
        led[i].Z = z;
    }
}

void LEDEngine::Set(uint32_t ledId, uint32_t val) {
    //led[ledId] = val;
}

void LEDEngine::Update(){
    for (int i = 0; i < led.size(); i++) {
        led[i].Update();
    }
}