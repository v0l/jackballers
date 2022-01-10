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
            <div className="footer">
                <p>
                    <small>
                        This site is not affilated with <a href="https://strike.me" className="txt-yellow">Strike</a> or <a href="https://twitter.com/JackMallers" className="txt-yellow">Jack Mallers</a>.
                        This is a parody site, all funds will be donated to <a href="https://hrf.org/" className="txt-yellow">HRF</a>.
                    </small>
                </p>
                <p>
                    <small>Made by <a href="https://github.com/v0l" className={"txt-yellow"}>v0l</a>.</small>
                </p>
            </div>
        </div>
    );
}

export default App;
