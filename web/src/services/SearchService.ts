import { config } from "../utils/config";
import { get } from "../utils/fetch";


export function GetHeat(){
    return get('/api/v1/heat');
}

export function GetInfo(){
    return get('/api/v1/info');
}


export async function Search(data: any) {
    // 拼接baseUrl并且处理/重复问题
    const baseUrl = config.FAST_API_URL;
    const url = `${baseUrl.replace(/\/$/, '')}/api/v1/search`;
    const response = await window.fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': "Bearer " + localStorage.getItem('token') || ''
        },
        body: JSON.stringify(data)
    });

    if(response.status !== 200){
        // 读取错误信息
        const text = await response.text();
        throw new Error(text);
    }

    const reader = response.body!.getReader();

    return {
        [Symbol.asyncIterator]() {
            return {
                async next() {
                    const { done, value } = await reader.read();
                    if (done) {
                        return { done: true, value: null };
                    }
                    const text = new TextDecoder("utf-8").decode(value);

                    const lines = text.split('\n').filter((line) => line.trim() !== '');

                    const values = [];
                    for (let i = 0; i < lines.length; i++) {
                        const line = lines[i].substring("data: ".length);
                        values.push(JSON.parse(line));
                    }
                    return {
                        done: false,
                        value: values,
                    };
                },
            };
        },
    }
}