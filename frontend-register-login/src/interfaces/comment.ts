import { Post } from "./post";
import { User } from "./user";

export interface Comment {
    id?: number;
    postId?: string;
    content?: string;
    username?: string;
    replyToId?: string; 
    replies?: Comment[]; 
    vote?: number;
    dateOfComment?: string; 
    isDeleted?:boolean;
  }