using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BagGomla.Enums;

namespace BagGomla.ViewModel
{
    public class Response<T> : Response where T : new()
    {
        public T DataResult { get; set; }
    }

    public class Response
    {
        public ResponseCode Code { get; set; }
        public bool IsSccuessCode { get { return Code == ResponseCode.Success; } }
        public string Message { get; set; }
        public Response()
        {
            Code = ResponseCode.Error;
        }
    }
}