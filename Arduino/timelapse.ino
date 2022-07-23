void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(7, INPUT_PULLUP); // UP
  pinMode(8, INPUT_PULLUP); // DOWN
  pinMode(9, INPUT_PULLUP); // LEFT
  pinMode(10, INPUT_PULLUP); // RIGHT
  pinMode(6, INPUT_PULLUP); // SET
  pinMode(11, INPUT_PULLUP); // PLAY_PAUSE
  Serial3.begin(115200);
}


#define MAX_VIDEOS 21


bool was_set_down = false;
bool was_up_down = false;
bool was_down_down = false;
bool was_left_down = false;
bool was_right_down = false;
bool was_play_pause_down = false;

int selected = 0;

int getXfromSelected(){
  return selected % 3;
  }

int getYfromSelected(){
  return (selected - (selected % 3)) / 3;
  }

int clamp(int val, int maxx, int minn){
  if (val >  maxx) return maxx;
  if (val < minn) return minn;
  return val;
  }


void loop() {
  // put your main code here, to run repeatedly:

//  SET KEY
  if (!digitalRead(6)) {
    if (!was_set_down){
      was_set_down = true;
      Serial.println("3");
      }
    }else {
      was_set_down = false;
    }


    //  PLAY KEY
  if (!digitalRead(11)) {
    if (!was_play_pause_down){
      was_play_pause_down = true;
      Serial.println("4");
      }
    }else {
      was_play_pause_down = false;
    }

//  UP KEY
  if (!digitalRead(7)) {
    if (!was_up_down){
      was_up_down = true;

      int x = getXfromSelected();
      int y = getYfromSelected();

      y -= 1;
      
      selected = clamp((y*3)+x, MAX_VIDEOS-1,0);
      
      
      Serial.println("1"+String(selected));
      delay(100);
      }
    }else {
      was_up_down = false;
    }

    
//  DOWN KEY
  if (!digitalRead(8)) {
    if (!was_down_down){
      was_down_down = true;

      int x = getXfromSelected();
      int y = getYfromSelected();

      y += 1;
      
      selected = clamp((y*3)+x, MAX_VIDEOS-1,0);
      
//      delay(100);
      Serial.println("1"+String(selected));
      delay(100);
      }
    }else {
      was_down_down = false;
    }
    
//  LEFT KEY
  if (!digitalRead(9)) {
    if (!was_left_down){
      was_left_down = true;

      selected = clamp(selected - 1, MAX_VIDEOS-1, 0);
      
//      delay(100);
      Serial.println("1"+String(selected));
delay(100);      
      }
    }else {
      was_left_down = false;
    }
    
//  RIGHT KEY
  if (!digitalRead(10)) {
    if (!was_right_down){
      was_right_down = true;
      selected = clamp(selected + 1, MAX_VIDEOS-1, 0);
      Serial.println("1"+String(selected));
      delay(100);
      }
    }else {
      was_right_down = false;
    }

    int i = 0;
    while (Serial3.available()){
  if (i == 0){
//    Serial.println();
    int x = Serial3.read();
    if (x == 1){
      Serial.println("21");
      
      }else if (x == 254){
        Serial.println("20");
        }
    }else {
      Serial3.read();
      
      }
  i++;
  }

   // delay(50);
}
