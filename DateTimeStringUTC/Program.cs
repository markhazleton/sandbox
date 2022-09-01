
using DateTimeStringUTC;

var IsoString = "2022-09-06T11:45:00-05:00";
Console.WriteLine(DateTime.Parse(IsoString).ToShortTimeString());
Console.WriteLine(IsoString.TimeStringFromIsoDateString());


IsoString = "2022-09-06T11:45:00-05:00";
var LocalDate = DateTime.Parse(IsoString);


Console.WriteLine(DateTime.Parse(IsoString).ToDallasTime().ToString("h:mm tt"));
Console.WriteLine(IsoString.TimeStringFromIsoDateString());




