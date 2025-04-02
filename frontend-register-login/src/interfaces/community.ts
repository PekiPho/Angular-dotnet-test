// export interface Community {
//     id:number;
//     name:string;
//     description?:string;
//     creationDate:string;
// }

import { Post } from "./post";

export interface Community {
    id: number;
    name: string;
    description?: string;
    creationDate: string;
    moderators: any | null; 
    subscribers: Subscriber[]; 
    posts: Post; 
    communityInfo:string|null;

}
  export interface Subscriber {
    id: number;
    username: string;
    password: string;
    email: string;
    moderator: any | null; 
    posts: any |null; 
    comments: any; 
    dateOfAccountCreated: string;
  }

  export interface CommunityToSend{

    name?:string,
    description?:string
  }
