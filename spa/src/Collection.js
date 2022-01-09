import "./Collection.css";
import {ItemGrid} from "./ItemGrid";
import {CollectionItem} from "./CollectionItem";
import {useEffect, useState} from "react";
import {Api} from "./Api";

export function Collection(props) {
    let [items, setItems] = useState([]);
    
    async function loadItems() {
        let api = new Api();
        
        let data = await api.getCollectionItems(props.data.id);
        setItems(data);
    }
    
    useEffect(() => {
        loadItems();    
    }, []);
    
    return (
      <div className="collection">
          <div className="header">
              <span className="title">{props.data.name}</span>
              <span className="description">{props.data.description}</span>
          </div>

          <ItemGrid>
              {items.sort((a,b) => a.number - b.number).map(a => <CollectionItem key={a.id} item={a}/>)}
          </ItemGrid>
      </div>  
    );
}