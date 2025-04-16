import { Comment } from "./comment";
import { Community } from "./community";
import { Media, MediaSmall } from "./media";
import { User } from "./user";

export interface Post {
    id: string;
    communityName: string; 
    title: string;
    description?: string;
    mediaIds: Media[] | null;
    comments?: string[];
    username: string;
    vote?: number;
    dateOfPost?: string; 
  }

  export interface PostToSend {
    community?: Community; 
    title: string;
    description?: string;
    media?: MediaSmall[];
    comments?: Comment[];
    user?: User;
    vote?: number;
    dateOfPost?: string; 
  }

  export interface Vote{
    votes:number;
    ratio:number;
  }