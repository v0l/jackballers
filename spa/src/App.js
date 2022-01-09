import './App.css';
import {useEffect, useState} from "react";
import {Collection} from "./Collection";
import {Api} from "./Api";

function App() {
    let [collections, setCollections] = useState([]);
    
    async function loadCollections() {
        let api = new Api();
        let data = await api.getCollections();
        setCollections(data);
    }
    
    useEffect(() => {
        loadCollections();
    }, []);
    
    return (
        <div className="app">
            <div className="header">
                <div className="title">Jack Ballers</div>
                <div className="links">
                    <div className="active">Collections</div>
                </div>
            </div>

            {collections.map(a => <Collection data={a} key={a.id}/>)}
        </div>
    );
}

export default App;
