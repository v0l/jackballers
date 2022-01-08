import './App.css';
import {useState} from "react";
import {CollectionItem} from "./CollectionItem";
import {ItemGrid} from "./ItemGrid";

function App() {
    const defaultItems = [
        {
            id: "ADF2A0E6-1C16-45BE-8C5B-F8C2F1CF0949",
            description: {
                name: "Baller",
                description: "A basic baller"
            },
            image: "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fpbs.twimg.com%2Fprofile_images%2F1402068570801119235%2FcDDDb7SP_400x400.jpg&f=1&nofb=1",
            rank: 1,
            fiatPrice: 1.0
        },
        {
            id: "D507AEE2-4FCE-4555-A61B-65648A2CF29C",
            description: {
                name: "Baller",
                description: "A basic baller"
            },
            image: "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fpbs.twimg.com%2Fprofile_images%2F1402068570801119235%2FcDDDb7SP_400x400.jpg&f=1&nofb=1",
            rank: 2,
            fiatPrice: 1.0
        },
        {
            id: "06AEE4F8-8069-47BF-8AAB-7AF8CCDCA67D",
            description: {
                name: "Baller",
                description: "A basic baller"
            },
            image: "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fpbs.twimg.com%2Fprofile_images%2F1402068570801119235%2FcDDDb7SP_400x400.jpg&f=1&nofb=1",
            rank: 3,
            fiatPrice: 1.0
        }
    ]
    let [items, setItems] = useState(defaultItems);

    return (<div className="app">
        <h1>Jack Ballers</h1>
        <ItemGrid>
            {items.map(a => <CollectionItem key={a.id} item={a}/>)}
        </ItemGrid>
    </div>);
}

export default App;
