﻿using Goodtocode.SemanticKernel.Core.Application.Abstractions;
using Goodtocode.SemanticKernel.Core.Application.Common.Exceptions;
using Goodtocode.SemanticKernel.Core.Domain.Author;

namespace Goodtocode.SemanticKernel.Core.Application.Author;

public class CreateAuthorCommand : IRequest<AuthorDto>
{
    public Guid Key { get; set; }
    public string? Name { get; set; }
}

public class CreateAuthorCommandHandler(ISemanticKernelContext context, IMapper mapper) : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IMapper _mapper = mapper;
    private readonly ISemanticKernelContext _context = context;

    public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        
        GuardAgainstEmptyName(request?.Name);
        
        // Persist Author
        var Author = new AuthorEntity() { Key = request!.Key == Guid.Empty ? Guid.NewGuid() : request!.Key };
        _context.Authors.Add(Author);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new CustomValidationException(
            [
                new("Key", "Key already exists")
            ]);
        }

        // Return session
        AuthorDto returnValue;
        try
        {
            returnValue = _mapper.Map<AuthorDto>(Author);
        }
        catch (Exception)
        {
            throw new CustomValidationException(
            [
                new("Key", "Key already exists")
            ]);
        }
        return returnValue;
    }

    private static void GuardAgainstEmptyName(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new CustomValidationException(
            [
                new("Name", "A name is required to get a response")
            ]);
    }
}