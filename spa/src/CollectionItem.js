import "./CollectionItem.css"
import {useEffect, useState} from "react";
import {PayWithStrike} from "./PayButton";
import {Api} from "./Api";
import QRCode from 'qrcode.react';

export function CollectionItem(props) {
    let [itemState, setItemState] = useState(ItemState.Buy);
    let [invoice, setInvoice] = useState();
    
    async function checkPaymentStatus() {
        let api = new Api();
        let invoiceStatus = await api.waitForInvoice(invoice.id);
        if(invoiceStatus.invoice.state === "PAID") {
            setItemState(ItemState.SaveJpg);
        }
    }
    
    async function handlePay(e) {
        let api = new Api();
        let invoice = await api.getInvoice(props.item.id);
        if(invoice) {
            setInvoice(invoice);
            setItemState(ItemState.Pay);
        }
    }
    
    function renderPaid() {
        
    }
    
    function renderResource() {
        
        switch(itemState) {
            case ItemState.Buy: {
                return <img alt="item-resource" className="nft" src={props.item.image}/>;
            }
            case ItemState.Pay: {
                return <QRCode className="resource" 
                   value={`lightning:${invoice.quote.lnInvoice}`} 
                   includeMargin={true}
                   style={{width: "300px", height: "300px"}}/>;
            }
            case ItemState.SaveJpg: {
                return renderPaid();
            }
        }
    }
    
    useEffect(() => {
        if(itemState === ItemState.Pay) {
            checkPaymentStatus();
        }
    }, [itemState]);
    
    return (
        <div className="item">
            <div className="title">{props.item.name} #{props.item.number}</div>
            <div className="resource">
                {renderResource()}
            </div>
            <div className="buy">
                <div>$ {props.item.fiatPrice.toFixed(2).toLocaleString()}</div>
                <PayWithStrike onClick={handlePay}/>
            </div>
        </div>
    );
}

const ItemState = {
    Buy: 1,
    Pay: 2,
    SaveJpg: 3
};