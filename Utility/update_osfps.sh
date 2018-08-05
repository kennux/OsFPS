# Helper script that can be used to update OsFPS when using it as git submodule
# The unity tk code (excluding examples) will be copied to Assets/OsFPS after pulling newest updates from git
# The git submodule should be stored in OsFPS.

rm -Rf Assets/OsFPS
cd OsFPS
git pull origin master
cd ..
cp -Rf OsFPS/Assets/OsFPS Assets/OsFPS