import './page.css'
import Bing from "./features/Bing";
import Github from "./features/Github";


export default function SearchPage() {

    const query = new URLSearchParams(window.location.search);
    const type = query.get('type') || "";
    if (type === "bing" || type === "") {
        return <Bing />
    }

    return <Github />
}