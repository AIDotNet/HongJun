﻿namespace HongJun.Service.Options;

public class JwtOptions
{
    public const string Name = "Jwt";
    
    /// <summary>
    /// 密钥
    /// </summary>
    public static string Secret { get; set; }

    /// <summary>
    /// 有效期
    /// </summary>
    public static int Effective { get; set; }
}