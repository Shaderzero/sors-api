using sors.Data.Dto.Incidents;
using sors.Data.Entities.Incidents;

namespace sors.Helpers
{
    public class ShaderMap
    {
        public static Measure MeasureMerge(Measure dbMeasure, MeasureForListDto dtoMeasure)
        {
            dbMeasure.Description = dtoMeasure.Description;
            dbMeasure.ExpectedResult = dtoMeasure.ExpectedResult;
            dbMeasure.DeadLine = dtoMeasure.DeadLine;
            dbMeasure.DeadLineText = dtoMeasure.DeadLineText;
            dbMeasure.Status = dtoMeasure.Status;
            return dbMeasure;
        }
    }
}
