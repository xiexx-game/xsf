# -*- coding: utf-8 -*-  

import codecs
# 防止中文乱码
import importlib
import os
import sys

import xlrd

importlib.reload(sys)

# 分割符
C_SPACE = ","
# 结束符
C_END = "\n"


IN_PATH = u"../../"
OUT_PATH = sys.argv[1]
OUT_SERVER_PATH = sys.argv[2]

# 获取指定后缀文件
def getFile(dir, suffix):
    res = []
    for root, directory, files in os.walk(dir):
        for filename in files:
            name, suf = os.path.splitext(filename)
            if suf == suffix:
                res.append(os.path.join(root, filename))
    return res


# 打开excel
def open_excel(file):
    try:
        data = xlrd.open_workbook(file)
        return data
    except Exception as e:
        print(str(e))


# 根据索引获取Excel表格中的数据 参数:file：Excel文件路径, colnameindex：表头列名所在行的索引, by_index：表的索引
def excel_table_byindex(sheet_data, order):
    nrows = sheet_data.nrows  # 行数
    ncols = sheet_data.ncols  # 列数
    rowlist = []
    for rownum in range(2, nrows):
        if sheet_data.cell(rownum, 0).value != "*":     # 该行未标记，不导出
            continue
        rowdata = sheet_data.row_values(rownum)
        if rowdata:
            collist = []
            for i in range(1, ncols):
                if sheet_data.cell(order, i).value != "*":      # 该列未标记，不导出
                    continue
                csv_data_type = sheet_data.cell(3, i).value.lower()
                if rownum > 4:
                    if csv_data_type == "uint" or csv_data_type == "int" or csv_data_type == "ulong":       # 如果是整形
                        cell_value = str(rowdata[i])
                        dot_index = cell_value.find('.')
                        if dot_index != -1:
                            first_cell_value = cell_value.split('.')[0]
                            collist.append(first_cell_value)
                        else:
                            collist.append(cell_value)
                    elif csv_data_type == "float":  # 如果是浮点数，只保留3位小数
                        cell_value = str(rowdata[i])
                        x_new = '{:.3f}'.format(float(cell_value))
                        collist.append(str(x_new))
                    else:
                        collist.append(str(rowdata[i]))
                else:
                    collist.append(str(rowdata[i]))

            if len(collist) > 0:
                rowlist.append(collist)
    return rowlist


# 保存csv文件
def savaToCSV(_file, _list, _path):
    print(u"savaToCSV:" + _file + " " + _path)
    filename = ""
    content = ""
    # 生成文件内容
    for collist in _list:
        for i in range(len(collist)):
            v = collist[i]
            vstr = ""
            # print k,v
            if isinstance(v, float) or isinstance(v, int):
                vstr = str(int(v))
            else:
                vstr = v
            if i > 0:
                content = content + C_SPACE
            content = content + vstr
        content = content + C_END

    # 生成文件后缀
    fname = os.path.basename(_file).split('.')
    filename = fname[0] + ".csv"

    # 写文件
    if len(filename) > 0 and len(content) > 0:
        filename = pathCombine(_path, filename)
        print(u"输出文件:" + filename)
        file_object = codecs.open(filename, 'w', "utf-8")
        file_object.write(content)
        file_object.close()


def main():
    filelist = getFile(IN_PATH, ".xlsx")
    if len(filelist) > 0:
        path = ""
        # 遍历文件生成csv
        for file in filelist:
            excel_data = open_excel(file)
            for sheet_name in excel_data.sheet_names():
                print("表单：", sheet_name)
                sheet_data = excel_data.sheet_by_name(sheet_name)
                datalist_Server = excel_table_byindex(sheet_data, 0)
                datalist_Client = excel_table_byindex(sheet_data, 1)
                if len(datalist_Client) > 0 and os.path.exists(OUT_PATH):
                    savaToCSV(sheet_name, datalist_Client, OUT_PATH)
                if len(datalist_Server) > 0 and os.path.exists(OUT_SERVER_PATH):
                    savaToCSV(sheet_name, datalist_Server, OUT_SERVER_PATH)
    else:
        print(u"没有找到任何excel文件！")


def pathCombine(path1: str, path2: str):
    if path1.endswith("/") or path1.endswith("\\"):
        return path1 + path2
    return path1 + "/" + path2


if __name__ == "__main__":
    main()
