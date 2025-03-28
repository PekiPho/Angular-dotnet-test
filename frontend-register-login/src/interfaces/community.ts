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
    moderators: any | null; // Adjust based on actual structure of moderators
    subscribers: Subscriber[]; // List of subscribers
    posts: Post; // Adjust as needed
  }

  export interface Subscriber {
    id: number;
    username: string;
    password: string;
    email: string;
    moderator: any | null; // Adjust based on your actual structure of moderators
    posts: any |null; // Adjust based on your actual data structure
    comments: any; // Adjust based on your actual data structure
    dateOfAccountCreated: string;
  }

  export interface CommunityToSend{

    name?:string,
    description?:string
  }
