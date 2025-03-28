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
    replyToId?: number; // Nullable for non-reply comments
    replyTo?: Comment; // Optional self-reference
    replies?: Comment[]; // List of replies
    vote: number;
    dateOfComment: string; // Date as ISO string
  }