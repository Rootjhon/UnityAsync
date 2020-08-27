#!/usr/bin/env python
# -*- encoding: utf-8 -*-
"""
@File    :   updateUPM.py
@Time    :   2020/08/01 14:13:54
@Author  :   JunQiang
@Contact :   354888562@qq.com
@Desc    :   
"""

# here put the import lib
import os
import shutil

def updateUPM(packageName: str, tag: str):
    os.system("git checkout -f master")
    shutil.move("Assets/{0}".format(packageName), ".git/")
    
    os.system("git checkout -f upm")
    os.system("git reset --hard")
    os.system("git clean -fd")
    os.system("git rm -rf --ignore-unmatch *")
    
    dir_name = ".git/{0}/".format(packageName)
    for d in os.listdir(dir_name):
        shutil.move(dir_name + d, "./")
    shutil.rmtree(dir_name)

    os.system("git add -A")
    os.system("git commit -m 'update upm to {0}'".format(tag))
    os.system("git tag {0}".format(tag))
    os.system("git push origin upm --tags")

    os.system("git checkout -f master")

if __name__ == "__main__":
    tag = "0.1.2"
    packageName = "UnityAsync"
    # updateUPM(packageName,tag)
    pass
