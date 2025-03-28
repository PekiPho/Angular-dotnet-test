import { Post } from "./post";

export interface Media {
    id: number;
    url: string;
    post?: Post; // Optional reference to Post
  }

  export interface MediaSmall{
    url:string;
    post?:Post;
  }