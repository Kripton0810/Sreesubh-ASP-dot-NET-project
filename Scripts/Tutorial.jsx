var CommentBox = React.createClass({
    render: function () {
        return (
            <div className="commentBox">
                Hello, world! This is viral jain.
          </div>
        );
    }
});
ReactDOM.render(
    <CommentBox />,
    document.getElementById('content')
);  