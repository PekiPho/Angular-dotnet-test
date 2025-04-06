import { Post } from "./post";

export interface Media {
    id: string;
    url: string;
    postId: string; 
  }

  export interface MediaSmall{
    url:string;
    post?:Post;
  }