using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace web_api_praktikum;

public class ValidationFailedResult : ObjectResult {
      public ValidationFailedResult(ModelStateDictionary modelState) : base(new FieldResultModel(modelState))
      {
            StatusCode = StatusCodes.Status400BadRequest;
      }
}

public class InvalidInputResult : ObjectResult {
      public InvalidInputResult(ModelStateDictionary modelState) : base(new FieldResultModel(modelState))
      {
            StatusCode = StatusCodes.Status422UnprocessableEntity;
      }
}

public class OtherErrorResult : ObjectResult {
      public OtherErrorResult(int code, ModelStateDictionary modelState) : base(new FieldResultModel(modelState))
      {
            StatusCode = code;
      }
}

public class FieldResultModel
{
      public string Message { get; }

      public Dictionary<string, string[]> Errors { get; }

      public FieldResultModel(ModelStateDictionary modelState)
      {
            Message = "Validation Failed";
            Errors = modelState.Where(e => e.Value?.Errors.Count > 0)
              .ToDictionary(
                  kvp => kvp.Key,
                  kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()!);
      }
}