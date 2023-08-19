using System;
using System.IO;

namespace FrugalCafe
{
    internal class TestEnumHelper
    {
        public static void Test()
        {
            var day = DayOfWeek.Saturday;

            var helper = EnumHelper<DayOfWeek>.Instance;

            string result = helper.FrugalToString(day);

            day = (DayOfWeek)8;

            result = helper.FrugalToString(day);
            result = helper.FrugalToString(day);

            FileAccess access = FileAccess.Read;

            result = EnumHelper<FileAccess>.Instance.FrugalToString(access);

            int count = 1000 * 1000;

            day = DayOfWeek.Saturday;

            PerfTest.MeasurePerf(
                () => { result = day.ToString(); }, 
                "Enum.ToString()", count);

            PerfTest.MeasurePerf(
                () => { result = EnumHelper<DayOfWeek>.Instance.FrugalToString(day); },
                "EnumHelper<T>.FrugalToString()", count);

            PerfTest.MeasurePerf(
                () => { result = access.ToString(); },
                "Flags Enum.ToString()", count);

            PerfTest.MeasurePerf(
                () => { result = EnumHelper<FileAccess>.Instance.FrugalToString(access); },
                "Flags EnumHelper<T>.FrugalToString()", count);
        }
    }
}
