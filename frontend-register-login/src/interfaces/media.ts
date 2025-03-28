import { Post } from "./post";

export interface Media {
    id: number;
    url: string;
    post?: Post; 
  }

  export interface MediaSmall{
    url:string;
    post?:Post;
  }