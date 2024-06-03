import { get } from "../utils/fetch";

export function getInfo(){
    return get('/api/v1/Users')
}