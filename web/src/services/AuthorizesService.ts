import { post } from "../utils/fetch";

export function Github(code:string){
    return post('/api/v1/Authorizes/Github?code='+code);
}