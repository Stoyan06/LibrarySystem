﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CloudinaryService

{

    private readonly Cloudinary _cloudinary;



    public CloudinaryService(IConfiguration config)

    {

        var account = new Account(

        config["Cloudinary:CloudName"],

        config["Cloudinary:ApiKey"],

        config["Cloudinary:ApiSecret"]);



        _cloudinary = new Cloudinary(account);

    }





    public async Task<string> UploadImageAsync(IFormFile file)

    {

        if (file == null || file.Length == 0)

            return null;

        try
        {

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams

            {

                File = new FileDescription(file.FileName, stream),

                Transformation = new Transformation()
                     .Width(300)
                     .Height(460)
                     .Crop("fit")



            };



            var uploadResult = await _cloudinary.UploadAsync(uploadParams);



            if (uploadResult == null || uploadResult.SecureUrl == null)

            {

                return null;

            }



            return uploadResult.SecureUrl.ToString();
        }
        catch (HttpRequestException ex)
        {
            return "NoInternetCustomWarning";
        }
    }

} 