
using Newtonsoft.Json;
namespace GAWeb
{
    public static class JSONConverter
    {
        public static string MatrixToJson(int[][] matrix)
        {
            return JsonConvert.SerializeObject(matrix);
        }
    }
}
