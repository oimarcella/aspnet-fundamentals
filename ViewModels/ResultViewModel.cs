using System.Collections.Generic;

namespace Blog.ViewModels
{
    public class ResultViewModel<T>
    {
        public ResultViewModel(T data, List<string> errors)
        {
            Data = data;
            Errors = errors;
        }
        public ResultViewModel(T data)
        {
            Data = data;
        }
        public ResultViewModel(List<string> errors)
        {
            Errors = errors;
        }
        public ResultViewModel(string error)
        {
            Errors.Add(error);
        }

        public ResultViewModel(string data, bool isError = true)
        {
            if (!isError)
            {
                Data = (T)(object)data;
                Errors = new List<string>();
            }
        }

        //ResultViewModel<T>.StatusCode.InternalServerError
        public ResultViewModel(StatusCode status)
        {
            if (status == StatusCode.Ok)
            {
                Message = GetDefaultMessage(status);
            }
            else
            {
                Errors.Add(GetDefaultMessage(status));
            }
        }

        public T Data { get; private set; }
        public List<string> Errors { get; private set; } = new();
        public string Message { get; private set; }

        public enum StatusCode
        {
            Ok = 200,
            BadRequest = 400,
            Unauthorized = 401,
            Forbidden = 403,
            NotFound = 404,
            InternalServerError = 500
        }

        private string GetDefaultMessage(StatusCode status)
        {
            return status switch
            {
                StatusCode.Ok => "Realizado com sucesso",
                StatusCode.BadRequest => "Requisição inválida",
                StatusCode.Unauthorized => "Não autorizado",
                StatusCode.Forbidden => "Acesso proibido",
                StatusCode.NotFound => "Recurso não encontrado",
                StatusCode.InternalServerError => "Falha interna",
                _ => "Erro desconhecido"
            };
        }
    }
}
