using System.Linq;
using System.Web.Http;
using Taw.Dcs.ScoreProcessor.Storage;
using Taw.Dcs.ScoreProcessor.Web.Models;

namespace Taw.Dcs.ScoreProcessor.Web.Controllers
{
    public class WriteCsvController : TableStorageWriteApiController
    {
        public WriteCsvController(ITableStorageWriteRepository writeRepository) 
            : base(writeRepository)
        {
        }

        // POST: api/Csv
        public void Post([FromBody] CsvInput value)
        {
            if (value.Separator == default(char))
            {
                value.Separator = ';';
            }

            SaveCsvLines(value.Csv.Split('\n').ToList(), value.Separator);
        }
    }
}

