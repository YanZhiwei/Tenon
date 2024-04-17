using Windows.Win32.Foundation;
using AutoMapper;
using Tenon.Infra.Windows.Win32.Extensions;
using Tenon.Infra.Windows.Win32.Models;
using CsWin32 = Windows.Win32;

namespace Tenon.Infra.Windows.Win32;

public sealed class Window
{
    private static readonly IMapper Mapper;

    static Window()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RECT, Rect>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.Width))
                .ForMember(dest => dest.IsEmpty, opt => opt.MapFrom(src => src.IsEmpty))
                .ForMember(dest => dest.X, opt => opt.MapFrom(src => src.X))
                .ForMember(dest => dest.Y, opt => opt.MapFrom(src => src.Y));
        });
        Mapper = config.CreateMapper();
    }

    public static Rect? GetRect(IntPtr intPtrHandle)
    {
        var hWnd = intPtrHandle.ToHWnd();
        if (hWnd == IntPtr.Zero || hWnd.IsNull) return null;
        if (CsWin32.PInvoke.GetWindowRect(hWnd, out var rect)) return Mapper.Map<RECT, Rect>(rect);
        return null;
    }

    public static Rectangle? GetRectangle(IntPtr intPtrHandle)
    {
        var hWnd = intPtrHandle.ToHWnd();
        if (hWnd == IntPtr.Zero || hWnd.IsNull) return null;
        if (CsWin32.PInvoke.GetWindowRect(hWnd, out var rect))
            return new Rectangle(rect.left, rect.top, rect.Width, rect.Height);
        return null;
    }
}