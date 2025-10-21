using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.Model
{
    public class BaseResponseModel<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T Data { get; set; }
    }
    public class BaseResponseModel
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public object? Data { get; set; }
    }
}
