import "./CollectionItem.css"
import {useEffect, useState, Fragment} from "react";
import {PayWithStrike} from "./PayButton";
import {Api} from "./Api";
import QRCode from 'qrcode.react';
import {Countdown} from "./Countdown";

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
        return (
            <div className="paid">
                <h3>Congrats!</h3>
                <p>
                    You are now the proud owner of {props.item.name} #{props.item.number}!
                </p>
                <p>
                    Download your unique NFT <a href={props.item.image} className="txt-yellow">here</a>
                </p>
                <small>All funds will be dontated to <a href={"https://hrf.org"} target={"_blank"} className="txt-yellow">HRF</a></small>
            </div>
        )
    }
    
    function renderResource() {
        
        switch(itemState) {
            case ItemState.Expired:
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
    
    function renderActions() {
        switch(itemState) {
            case ItemState.Expired:
            case ItemState.Buy: {
                return (
                    <Fragment>
                        <div>$ {props.item.fiatPrice.toFixed(2).toLocaleString()}</div>
                        <PayWithStrike onClick={handlePay}/>
                    </Fragment>
                );
            }
            case ItemState.Pay: {
                return (
                    <Fragment>
                        <Countdown to={invoice.quote.expiration} onEnd={() => setItemState(ItemState.Expired)}/>
                        {(parseFloat(invoice.quote.sourceAmount.amount) * 1.0e8).toLocaleString()} SATS
                    </Fragment>
                );
            }
        }
        return null;
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
                {renderActions()}
            </div>
        </div>
    );
}

const ItemState = {
    Buy: 1,
    Pay: 2,
    SaveJpg: 3,
    Expired: 4
};