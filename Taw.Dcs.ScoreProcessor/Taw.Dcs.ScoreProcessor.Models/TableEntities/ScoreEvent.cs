using System;
using System.Globalization;
using Microsoft.WindowsAzure.Storage.Table;

namespace Taw.Dcs.ScoreProcessor.Models.TableEntities
{
    [Serializable]
    public class ScoreEvent : TableEntity
    {
        //public ScoreEvent() { } //Necessary for table deser

        public static ScoreEvent Create(string readLine, char columnSeparator)
        {
            if (readLine.Length == 0 || readLine.StartsWith("#"))
            {

                return new ScoreEvent { Error = "Readline is null or escaped" };
            }
            const string runTimePattern = "yy-MM-dd_HH-mm-ss";
            const string timePattern = "HH:mm:ss";
            var items = readLine.Split(columnSeparator);
            if (items.Length != 16)
            {
                throw new InvalidAmountOfColumnsException("The number of columns in the CSV does not match!");
            }
            DateTime date, time;
            int times, score;
            if (!DateTime.TryParseExact(items[1], runTimePattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return new ScoreEvent { Error = "Could not parse runTime from " + items[1] };
            }
            if (!DateTime.TryParseExact(items[2], timePattern, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out time))
            {
                return new ScoreEvent { Error = "Could not parse time from >" + items[2] + "<" };
            }
            if (!int.TryParse(items[14], out times))
            {
                return new ScoreEvent { Error = "Could not parse times from " + items[14] };
            }
            if (!int.TryParse(items[15], out score))
            {
                return new ScoreEvent { Error = "Could not parse score from " + items[15] };
            }
            var scoreEvent = new ScoreEvent
            {
                Csv = readLine,
                Separator = columnSeparator.ToString(),

                RunTime = date,
                Time = time,
                GameName = items[0],
                PlayerName = items[3],
                TargetPlayerName = items[4],
                ScoreType = items[5],
                PlayerUnitCoalition = items[6],
                PlayerUnitCategory = items[7],
                PlayerUnitType = items[8],
                PlayerUnitName = items[9],
                TargetUnitCoalition = items[10],
                TargetUnitCategory = items[11],
                TargetUnitType = items[12],
                TargetUnitName = items[13],
                Times = times,
                Score = score,
                PartitionKey = items[0] + ";" + date.ToShortDateString(),
                RowKey = Guid.NewGuid().ToString(),
                Error = ""
            };
            return scoreEvent;
        }

        public override string ToString()
        {
            return $"At {Time}, {PlayerName} scored a {ScoreType} against {TargetPlayerName} in {TargetUnitName} for {Times * Score} points.";
        }

        public string GameName { get; set; }
        public DateTime RunTime { get; set; }
        public DateTime Time { get; set; }
        public string PlayerName { get; set; }
        public string TargetPlayerName { get; set; }
        public string ScoreType { get; set; }
        public string PlayerUnitCoalition { get; set; }
        public string PlayerUnitCategory { get; set; }
        public string PlayerUnitType { get; set; }
        public string PlayerUnitName { get; set; }
        public string TargetUnitCoalition { get; set; }
        public string TargetUnitCategory { get; set; }
        public string TargetUnitType { get; set; }
        public string TargetUnitName { get; set; }
        public int Times { get; set; }
        public int Score { get; set; }
        public string Target => !string.IsNullOrEmpty(TargetPlayerName) ? TargetPlayerName : TargetUnitName;
        public string Csv { get; set; }
        public string Separator { get; set; }
        public string Error { get; set; }
    }
}