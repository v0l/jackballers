import "./PayButton.css"

export function PayWithStrike(props) {
    
    function handleClick(e) {
        if(typeof props.onClick === "function") {
            props.onClick(e);
        }
    }
    
    return (
      <div className="pay-btn" onClick={handleClick}>
          <img alt="icon" src="/favicon.svg"/>
          <div>Strike Pay</div>
      </div>  
    );
}