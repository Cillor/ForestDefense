using System;

public class SaveState
{
    public int batterysInInventory = 0;
    public float wattsStored = (float)Math.Pow(10, 10) * 99;
    public float stamina = 100;
    public int coffeeQuantity = 0;
    public float coffeeAddiction = 0;
    public float poison = 0;
    public int metalAmount = 0;
    public int recharges = 0;

    public int actualWave = 0;
    public bool[] waveUnlocked = new bool[10];
    public bool[] gunIdUnlocked = new bool[8];
    public int equipedGunId = 0;

    public bool firstStartOcurred;
    public bool endGameOcurred;
}
