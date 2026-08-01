// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace cCoder.ContentManagement.Models.Results;

public class OperationResult<T> : OperationResult
{
    private string id;

    [Key]
    public override string Id
    {
        get
        {
            if (id != null)
            {
                return id;
            }

            try
            {
                return Item is null
                    ? null
                    : ((dynamic)Item).Id?.ToString();
            }
            catch
            {
                return null;
            }
        }
        set =>
            id = value;
    }

    public T Item { get; set; }
}