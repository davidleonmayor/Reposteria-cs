# DTOs and validation

DTOs live in `Api/Dtos/` and are used as request models for controllers.

## Goals
- Prevent overposting (do not bind EF entities from request bodies).
- Validate inputs using DataAnnotations (`[Required]`, `[MaxLength]`, `[Range]`, etc.).

## Common conventions
- Controllers normalize user-provided strings with `StringNormalization.Clean`.
- Some DTO fields use special semantics (example: `ProductCreateDto.CategoryId == 0` -> "Uncategorized").
