//Headers------------------------------------------------------------------------------------------
#include <LiquidCrystal.h>
//Definitions--------------------------------------------------------------------------------------
LiquidCrystal lcd(2, 3, 4, 5, 6, 7);
int IR1 = 8;
int IR2 = 9;
int IR3 = 10;
int IR4 = 11;
//Functions----------------------------------------------------------------------------------------
void Beep(int No, int Delay);

//Variables----------------------------------------------------------------------------------------
char STS1,STS2,STS3,STS4;
//-------------------------------------------------------------------------------------------------
//#################################################################################################
void setup() 
{
  //LED------------------------------------
  pinMode(LED_BUILTIN, OUTPUT);Beep(3,100);
  
  pinMode(IR1, INPUT);
  pinMode(IR2, INPUT);
  pinMode(IR3, INPUT);
  pinMode(IR4, INPUT);
  
  //LCD------------------------------------
  lcd.begin(16, 2);
  lcd.setCursor(0,0);lcd.print("    Welcome!    ");
  lcd.setCursor(0,1);lcd.print("   V.E.S.I.T.   ");
  delay(2000);
  lcd.setCursor(0,0);lcd.print("Project Done By.");
  lcd.setCursor(0,1);lcd.print("Hanisha         ");
  delay(2000);
  lcd.setCursor(0,0);lcd.print("Meet            ");
  lcd.setCursor(0,1);lcd.print("Krishna         ");
  delay(2000); 
  lcd.setCursor(0,0);lcd.print("Under Guidance..");
  lcd.setCursor(0,1);lcd.print("A.Pf.Sunita Sahu");
  delay(2000);    
  lcd.clear();
  lcd.setCursor(0,0);lcd.print("Slot Status:    ");
  //Serial---------------------------------
  Serial.begin(9600);
 
}

void loop() 
{
  if(digitalRead(IR1)==HIGH)STS1='0';else STS1='1';
  if(digitalRead(IR2)==HIGH)STS2='0';else STS2='1';
  if(digitalRead(IR3)==HIGH)STS3='0';else STS3='1';
  if(digitalRead(IR4)==HIGH)STS4='0';else STS4='1';

  lcd.setCursor(0,1);
  lcd.print(STS1);lcd.print(" ");
  lcd.print(STS2);lcd.print(" ");
  lcd.print(STS3);lcd.print(" ");
  lcd.print(STS4);lcd.print(" ");
  
  if(Serial.available()>0)
  {
    Serial.read();
    Serial.print('L');
    Serial.print('I');
    Serial.write(4);
    Serial.print(STS1);
    Serial.print(STS2);
    Serial.print(STS3);
    Serial.print(STS4);
    Beep(1,100); 
  }
  
}
//#################################################################################################
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
void Beep(int No, int Delay)
{
  unsigned char tNo;

  for(tNo=1;tNo<=No;tNo++)  
  {
    digitalWrite(LED_BUILTIN, LOW );
    delay(Delay);
    digitalWrite(LED_BUILTIN, HIGH);
    delay(Delay);
  }
}
//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


