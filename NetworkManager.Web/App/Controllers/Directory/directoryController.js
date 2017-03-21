(function () {
    'use strict';

    angular
        .module('NetworkManager')
        .controller('DirectoryController', DirectoryController);

    DirectoryController.$inject = ['$scope', '$q', 'DirectoryService', 'errorHandler', '$modal'];

    function DirectoryController($scope, $q, DirectoryService, errorHandler, $modal) {
        (function startup() {
            var directory = DirectoryService.getDirectory();
            var currentPath = {};
            $q.all([directory]).then(function (result) {
                if (result[0].data != null) {
                    $scope.directory = []; $scope.drives = []; $scope.directory = result[0].data;
                    var currentDrivesList = $scope.drives.concat($scope.directory[0].drives);
                    $scope.drives = currentDrivesList;
                    currentPath = $scope.directory[0].path;
                }
            }, function (reason) {
                errorHandler.logServiceError('DirectoryController', reason);
            }, function (update) {
                errorHandler.logServiceNotify('DirectoryController', update);
            });
            setdirectoriesInfo(currentPath);

        })();

        function setdirectoriesInfo(currentPath) {
            var directoriesInfo = DirectoryService.getDirectoriesInfo(currentPath);
            $q.all([directoriesInfo]).then(function (result) {
                if (result != null) {
                    $scope.directory = [];
                    $scope.directory.push(result[0].data);
                }
            }, function (reason) {
                errorHandler.logServiceError('DirectoryController', reason);
            }, function (update) {
                errorHandler.logServiceNotify('DirectoryController', update);
            });
        };

        $scope.gotoDir = function (path) { DirectoryService.getDirectory(path); };
        $scope.directory = [];
        $scope.drives = [];
        $scope.Commands = {
            checkDir: function (dir) {
                var path = { path: dir.path };
                DirectoryService.getDirectory(path).then(
                    function (result) {
                        if (result.data != null) {
                            $scope.directory = []; $scope.drives = [];
                            $scope.directory.push(result.data[0]);
                            $scope.drives = $scope.directory[0].drives;
                            setdirectoriesInfo(path);
                        }
                    },
                    function (response) {
                        console.log(response);
                    });
            },
            openRoot: function (dir) {
                var path = {path:dir.rootPath};
                DirectoryService.getDirectory(path).then(
                    function (result) {
                        if (result.data != null) {
                            $scope.directory = []; $scope.drives = [];
                            $scope.directory.push(result.data[0]);
                            $scope.drives = $scope.directory[0].drives;
                            setdirectoriesInfo(path);
                        }
                    },
                    function (response) {
                        console.log(response);
                    });
            },
            openParrent: function (dir) {
                var path = { path: dir.parrentPath };
                DirectoryService.getDirectory(path).then(
                    function (result) {
                        if (result.data != null) {
                            $scope.directory = []; $scope.drives = [];
                            $scope.directory.push(result.data[0]);
                            $scope.drives = $scope.directory[0].drives;
                            setdirectoriesInfo(path);
                        }
                    },
                    function (response) {
                        console.log(response);
                    });
            }
        };
        $scope.Queries = {
            getDirectoriesInfo: function (path) {
                DirectoryService.getDirectoriesInfo(path);
            },
            getDirectory: function (path) {
                DirectoryService.getDirectory(path);
            }
        };
        $scope.Actions = {
            openDir: function (dir) {
                $scope.Commands.checkDir(dir);
            },
            openRoot: function (dir) {
                $scope.Commands.openRoot(dir);
            },
            openParrent: function (dir) {
                $scope.Commands.openParrent(dir);
            }
        },
        $scope.Modals = {
        }
    };
})
();