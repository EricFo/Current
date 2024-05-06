public class MeterConfig
{    
    //对应 MAXI MEGA MAJOR MINI
    public int[] MeterBase = new int[] { 10000, 800, 30, 10 };
    //对应 MAXI MEGA MAJOR MINI,每次点Spin增加一点,需要结合ExchangeRate进行计算
    public int[] Increment = new int[] { 2, 2, 0, 0 };
    //对应 MAXI MEGA MAJOR MINI,换算公式 MeterBase += Increment / ExchangeRate 
    public int[] ExchangeRate = new int[] { 1000, 1000, 1000, 1000 };    
}
