import "./ItemGrid.css"

export function ItemGrid(props) {
    return (
        <div className="item-grid">
            {props.children}
        </div>
    )
}