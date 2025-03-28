import { Post } from "./post";
import { User } from "./user";

export interface Comment {
    id: number;
    postId: number;
    postCommunityId: number;
    post: Post;
    content: string;
    userId: number;
    user: User;
    replyToId?: number; 
    replyTo?: Comment; 
    replies?: Comment[]; 
    vote: number;
    dateOfComment: string; 
  }