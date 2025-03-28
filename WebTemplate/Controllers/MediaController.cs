namespace WebTemplate.Controllers;


public class MediaController:ControllerBase{

    public IspitContext Context{get;set;}

    public MediaController(IspitContext context){
        Context=context;
    }

    [HttpGet("UploadMedia")]
    public async Task<ActionResult> UploadMedia([FromForm] List<IFormFile> mediaFiles){
        if(mediaFiles==null || mediaFiles.Count==0){
            return BadRequest("No data sent");
        }


        var mediaList=new List<Media>();

        var imageCount=mediaFiles.Count(c=>c.ContentType.StartsWith("image"));
        var videoCount=mediaFiles.Count(c=>c.ContentType.StartsWith("video"));

        if(imageCount>0 && videoCount>0){
            return BadRequest("You can only upload 1 video or up to 10 images");
        }
        if(imageCount>10)
            return BadRequest("Only up to 10 images can be uploaded");
        if(videoCount>1){
            return BadRequest("You can only upload 1 video to a post");
        }

        foreach(var media in mediaFiles){

            if(media.ContentType.StartsWith("video")){
                if(media.Length>1024*1024*1024)
                    return BadRequest("Only up to 1GB video can be uploaded");

                //duration should be done in frontend because i dont know how to do it here tbh
            }

            if(media.ContentType.StartsWith("image")){
                if(media.Length>15*1024*1024)
                    return BadRequest("All images must be under 15MB");
            }
        }


        string upload=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","uploads");
        if(!Directory.Exists(upload))
            Directory.CreateDirectory(upload);

        foreach(var media in mediaFiles){
            string name=$"{Guid.NewGuid()}_{media.FileName}";
            string path=Path.Combine(upload,name);

            using(var stream= new FileStream(path,FileMode.Create)){
                await media.CopyToAsync(stream);
            }

            var file= new Media{
                Url=$"/uploads/{name}"
            };

            await Context.Media.AddAsync(file);
            mediaList.Add(file);
        }

        await Context.SaveChangesAsync();
        return Ok(mediaList);
    }
}