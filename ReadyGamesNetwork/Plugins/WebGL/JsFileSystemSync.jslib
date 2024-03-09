mergeInto(LibraryManager.library, {

    JsFileSystemSync: function () {
        FS.syncfs(false, function (err) {});
    },
    
});