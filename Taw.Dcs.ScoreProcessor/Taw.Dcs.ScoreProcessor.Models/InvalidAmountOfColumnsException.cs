using System;

namespace Taw.Dcs.ScoreProcessor.Models
{
    public class InvalidAmountOfColumnsException : ApplicationException
    {
        public InvalidAmountOfColumnsException(string message)
            : base(message)
        {

        }
    }
}