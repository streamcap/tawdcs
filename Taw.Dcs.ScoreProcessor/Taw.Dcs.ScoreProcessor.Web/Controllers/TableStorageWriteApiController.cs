using System.Collections.Generic;
using System.Web.Http;
using Taw.Dcs.ScoreProcessor.Storage;
using Taw.Dcs.ScoreProcessor.Web.Models;

namespace Taw.Dcs.ScoreProcessor.Web.Controllers
{
    public abstract class TableStorageWriteApiController : ApiController
    {
        private readonly ITableStorageWriteRepository _writeRepository;
        protected readonly CollectionSink CollectionSink;

        protected TableStorageWriteApiController(ITableStorageWriteRepository writeRepository)
        {
            _writeRepository = writeRepository;
            CollectionSink = new CollectionSink();
        }

        protected void SaveCsvLines(ICollection<string> eventLines, char separator)
        {
            _writeRepository.InsertScoreEvents(eventLines, separator);
        }
    }
}