using System.ComponentModel.DataAnnotations;

namespace GameApi.DTOs;

public class CreateCharacterRequest
{
    [Required(ErrorMessage = "Tên linh thú là bắt buộc")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên linh thú phải từ 2 đến 50 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nguyên tố là bắt buộc")]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Nguyên tố phải từ 1 đến 30 ký tự")]
    public string Class { get; set; } = string.Empty;
}

public class UpdateCharacterRequest
{
    [Required(ErrorMessage = "Tên linh thú là bắt buộc")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên linh thú phải từ 2 đến 50 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nguyên tố là bắt buộc")]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Nguyên tố phải từ 1 đến 30 ký tự")]
    public string Class { get; set; } = string.Empty;

    // int? + [Required]: thiếu trường thì bị chặn 400, không bị điền 0.
    [Required(ErrorMessage = "Cấp là bắt buộc")]
    [Range(1, 100, ErrorMessage = "Cấp phải từ 1 đến 100")]
    public int? Level { get; set; }

    [Required(ErrorMessage = "Máu là bắt buộc")]
    [Range(0, 100000, ErrorMessage = "Máu phải từ 0 đến 100000")]
    public int? Hp { get; set; }
}

public record CharacterResponse(int Id, string Name, string Class, int Level, int Hp, int AccountId);
