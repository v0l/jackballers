import {useEffect, useState} from "react";

export function Countdown(props) {
    let [time, setTime] = useState();
    
    function updateTime() {
        let expire = new Date(props.to).getTime();
        let now = new Date().getTime();
        let v = expire > now ? expire - now : 0.0;
        if(v === 0.0 && typeof props.onEnd === "function") {
            props.onEnd();
        }
        setTime(`${(v / 1000.0).toFixed(0)}s`);
    }
    
    useEffect(() => {
        updateTime();
        let t = setInterval(updateTime, 500);
        return () => clearInterval(t);
    }, []);
    
    return (
        <div className="txt-center">
            {time}
        </div>
    );
}