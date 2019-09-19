<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="html" indent="yes"/>

    <xsl:template match="/">
        <div class="wrap_history">
            <table class="responsive">
                <tr>
                    <th class="text_left">
                        Mã
                    </th>
                    <th class="text_left">
                        Sản phẩm
                    </th>
                    <th class="text_center">
                        Ngày đặt hàng
                    </th>
                    <th class="text_center">
                        Tình trạng hóa đơn
                    </th>
                    <th class="text_right">
                        Tổng số tiền
                    </th>
                </tr>
                <xsl:apply-templates select="/ProductList/Product"></xsl:apply-templates>
            </table>
        </div>
    </xsl:template>
    <xsl:template match="Product">
        <tr>
            <td class="code_history">
                <xsl:value-of select="OrderCode"></xsl:value-of>
            </td>
            <td class="name_history">
                <a class="trainsion opacity">
                    <xsl:attribute name="href">
                        <xsl:value-of select="Url"></xsl:value-of>
                    </xsl:attribute>
                    <xsl:attribute name="target">
                        <xsl:value-of select="Target"></xsl:value-of>
                    </xsl:attribute>
                    <xsl:attribute name="title">
                        <xsl:value-of select="Title"></xsl:value-of>
                    </xsl:attribute>
                    <xsl:value-of select="Title"></xsl:value-of>
                </a>
                <xsl:value-of select="EditLink" disable-output-escaping="yes"></xsl:value-of>
                <!--<xsl:value-of select="AdditionalService"></xsl:value-of>-->
                <xsl:if test="Color != ''">
                    Màu: <xsl:value-of select="Color"></xsl:value-of>
                </xsl:if>
            </td>
            <td class="date_history text_center">
                <xsl:value-of select="OrderDate"></xsl:value-of>
            </td>
            <td class="thing_history text_center">
                <xsl:value-of select="OrderStatus"></xsl:value-of>
            </td>
            <td class="sum_history text_right">
                <span>
                    <xsl:value-of select="OrderTotal"></xsl:value-of>
                </span>
                <!--<xsl:value-of select="AdditionalServicePrice"></xsl:value-of>-->
            </td>
        </tr>
    </xsl:template>
</xsl:stylesheet>