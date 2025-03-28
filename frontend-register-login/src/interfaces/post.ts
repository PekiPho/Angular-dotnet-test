import { Comment } from "./comment";
import { Community } from "./community";
import { Media, MediaSmall } from "./media";
import { User } from "./user";

export interface Post {
    id: number;
    communityName: string; 
    title: string;
    description?: string;
    mediaIds: string[];
    comments?: number[];
    username: string;
    vote?: number;
    dateOfPost?: string; // Date as ISO string
  }

  export interface PostToSend {
    community?: Community; 
    title: string;
    description?: string;
    media?: MediaSmall[];
    comments?: Comment[];
    user?: User;
    vote?: number;
    dateOfPost?: string; // Date as ISO string
  }