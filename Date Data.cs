using System;
using System.Collections.Generic;
using System.Linq;

public class Calender
{
	// float year.month, string[] ("startTime hour.minute", "endTime hour.minute" "link", ...)
	Dictionary<float, List<string>> cal = new Dictionary<float, List<string>>();

	// Get all the time periods in a dictionary form startTime : endTime
	public Dictionary<float, float> getTimesPeriods(List<string> info)
	{
		float startTime = 0.0;
		float endTime = 0.0;
		Dictionary<float, float> timePeriods = new Dictionary<float, float>();
		for (int i = 0; i < info.Length; i += 3) {
			startTime = float.Parse(info[i]);
			endTime = float.Parse(info[i+1]);
			timePeriods[startTime] = endTime;
		}
		return timePeriods;
	}

	// Get a list of time periods in list string
	public List<string> orderedTimePeriods(Dinctionary<float, float>  timePeriods)
	{
		List<float> periods = new List<float>();
		int i;
		foreach (KeyValuePair<float, float> period in timePeriods) {
			i = 0;
			if (periods.count == 0) {
				periods[0] = 
			}
			else {
				while (i < periods.count) {
					if 
				}
			}
		}
	}

	// Find the link given a time period inside of the info list string
	public string[] getLink(List<string> info, List<string> timePeriod)
	{
		List<string> testPeriod = new List<string>();
		for (int i = 0; i < info.Length; i += 3) {
			testPeriod[0] = info[i];
			testPeriod[1] = info[i + 1];
			if (testPeriod.SequenceEqual(timePeriod)) {
				return info[i + 2];
			}
		}
		return null;
	}

	// have the hour.minute time go to PM and AM time in a string
	public string[] clockTime(string[] strTime)
	{
		float time = float.Parse(strTime);
		int hours = (int)time;
		int minutes = (int)((time - (float)(int)time)*100)
		if (int(hours/12) == 0) {
			return $"A.M. {hours}:{minutes}";
		}
		else if (int(hours/12) == 1) {
			return $"P.M. {hours-12}{minutes}";
		}
	}
}
